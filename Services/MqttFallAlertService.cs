using System.Text;
using System.Text.Json;
using FallDetectionMonitor.Data;
using FallDetectionMonitor.Models;
using Microsoft.EntityFrameworkCore;
using MQTTnet;

namespace FallDetectionMonitor.Services
{
    public class MqttFallAlertService : BackgroundService
    {
        private const string MqttBroker = "broker.hivemq.com";
        private const int MqttPort = 1883;
        private const string MqttTopic = "fall-detection-monitor/irfan/alerts";
        private const string MqttTelemetryTopic = "fall-detection-monitor/irfan/telemetry";

        private readonly IDbContextFactory<FallDetectionDbContext> _dbFactory;
        private readonly ILogger<MqttFallAlertService> _logger;

        private IMqttClient? _mqttClient;

        public MqttFallAlertService(
            IDbContextFactory<FallDetectionDbContext> dbFactory,
            ILogger<MqttFallAlertService> logger)
        {
            _dbFactory = dbFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var mqttFactory = new MqttClientFactory();
            _mqttClient = mqttFactory.CreateMqttClient();

            _mqttClient.ApplicationMessageReceivedAsync += async args =>
            {
                string topic = args.ApplicationMessage.Topic;
                string payload = args.ApplicationMessage.ConvertPayloadToString();

                _logger.LogWarning( "MQTT RECEIVED: Retain={Retain}, Topic={Topic}, Payload={Payload}",args.ApplicationMessage.Retain,topic,    payload);

                if (args.ApplicationMessage.Retain)
                {
                    _logger.LogWarning("Ignored retained MQTT message.");
                    return;
                }

                _logger.LogInformation(
                    "MQTT message received. Topic: {Topic}, Payload: {Payload}",
                    topic,
                    payload);

                await HandleMqttMessageAsync(topic, payload, stoppingToken);
            };

            var mqttOptions = new MqttClientOptionsBuilder()
                .WithClientId($"FallDetectionMonitor_{Guid.NewGuid():N}")
                .WithTcpServer(MqttBroker, MqttPort)
                .WithCleanSession()
                .Build();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (!_mqttClient.IsConnected)
                    {
                        _logger.LogInformation("Connecting to MQTT broker {Broker}:{Port}", MqttBroker, MqttPort);

                        await _mqttClient.ConnectAsync(mqttOptions, stoppingToken);

                        var subscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                            .WithTopicFilter(f => f
                                .WithTopic(MqttTopic)
                                .WithAtLeastOnceQoS())
                            .WithTopicFilter(f => f
                                .WithTopic(MqttTelemetryTopic)
                                .WithAtLeastOnceQoS())
                            .Build();

                        await _mqttClient.SubscribeAsync(subscribeOptions, stoppingToken);

                        _logger.LogInformation("Subscribed to MQTT topic: {Topic}", MqttTopic);
                        _logger.LogInformation("Subscribed to MQTT topic: {Topic}", MqttTelemetryTopic);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "MQTT connection/subscription failed. Retrying soon...");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        private async Task HandleMqttMessageAsync(string topic, string payload, CancellationToken cancellationToken)
        {
            MqttDeviceMessage? message;

            try
            {
                message = JsonSerializer.Deserialize<MqttDeviceMessage>(
                    payload,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Invalid MQTT JSON payload: {Payload}", payload);
                return;
            }

            if (message == null)
            {
                _logger.LogWarning("MQTT message was empty.");
                return;
            }

            if (string.IsNullOrWhiteSpace(message.DeviceCode))
            {
                _logger.LogWarning("MQTT message missing deviceCode.");
                return;
            }

            if (string.IsNullOrWhiteSpace(message.EventType))
            {
                _logger.LogWarning("MQTT message missing eventType.");
                return;
            }

            await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);

            var device = await db.WearableDevices
                .FirstOrDefaultAsync(d => d.DeviceCode == message.DeviceCode, cancellationToken);

            if (device == null)
            {
                _logger.LogWarning("No wearable device found with DeviceCode: {DeviceCode}", message.DeviceCode);
                return;
            }

            string eventType = message.EventType.Trim().ToLower();

            if (topic == MqttTelemetryTopic || eventType == "telemetry")
            {
                await SaveTelemetryAsync(db, device.Id, message, cancellationToken);
                return;
            }

            if (eventType == "fall_detected")
            {
                await SaveFallDetectedAsync(db, device.Id, message, cancellationToken);
                return;
            }

            if (eventType == "alarm_cleared")
            {
                await SaveAlarmClearedAsync(db, device.Id, message, cancellationToken);
                return;
            }

            _logger.LogWarning("Unknown MQTT eventType: {EventType}", message.EventType);
        }

        private async Task SaveFallDetectedAsync(
            FallDetectionDbContext db,
            int wearableDeviceId,
            MqttDeviceMessage message,
            CancellationToken cancellationToken)
        {
            var existingActiveAlert = await db.AlertEvents.AnyAsync(
        a => a.WearableDeviceId == wearableDeviceId &&
             a.IsResolved == false &&
             a.AlertType == "Fall Detected",
        cancellationToken);

            if (existingActiveAlert)
            {
                _logger.LogInformation(
                    "Ignored duplicate fall alert for device ID {WearableDeviceId}",
                    wearableDeviceId);

                return;
            }
            var reading = new SensorReading
            {
                WearableDeviceId = wearableDeviceId,
                RecordedAt = DateTime.Now,

                HeartRateBpm = 0,

                PressureHpa = message.PressureHpa ?? 0,
                AltitudeM = message.AltitudeM ?? 0,

                AccelX = message.AccelX ?? 0,
                AccelY = message.AccelY ?? 0,
                AccelZ = message.AccelZ ?? 0,

                GyroX = message.GyroX ?? 0,
                GyroY = message.GyroY ?? 0,
                GyroZ = message.GyroZ ?? 0,

                FallDetected = true,
                LedActive = message.LedActive ?? true,
                BuzzerActive = message.BuzzerActive ?? true,
                MlFallScore = message.MlFallScore ?? 1.0,
                MlTriggered = true
            };

            db.SensorReadings.Add(reading);

            var alert = new AlertEvent
            {
                WearableDeviceId = wearableDeviceId,
                CreatedAt = DateTime.Now,
                AlertType = "Fall Detected",
                Message = "Fall detected from MQTT device.",
                IsResolved = false
            };

            db.AlertEvents.Add(alert);

            await db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Fall detected MQTT event saved for device ID {WearableDeviceId}", wearableDeviceId);
        }

        private async Task SaveAlarmClearedAsync(
            FallDetectionDbContext db,
            int wearableDeviceId,
            MqttDeviceMessage message,
            CancellationToken cancellationToken)
        {
            var reading = new SensorReading
            {
                WearableDeviceId = wearableDeviceId,
                RecordedAt = DateTime.Now,

                HeartRateBpm = 0,

                PressureHpa = message.PressureHpa ?? 0,
                AltitudeM = message.AltitudeM ?? 0,

                AccelX = message.AccelX ?? 0,
                AccelY = message.AccelY ?? 0,
                AccelZ = message.AccelZ ?? 9.81,

                GyroX = message.GyroX ?? 0,
                GyroY = message.GyroY ?? 0,
                GyroZ = message.GyroZ ?? 0,

                FallDetected = false,
                LedActive = false,
                BuzzerActive = false,
                MlFallScore = message.MlFallScore ?? 0,
                MlTriggered = false
            };

            db.SensorReadings.Add(reading);

            var activeAlerts = await db.AlertEvents
                .Where(a => a.WearableDeviceId == wearableDeviceId && a.IsResolved == false)
                .ToListAsync(cancellationToken);

            foreach (var alert in activeAlerts)
            {
                alert.IsResolved = true;
                alert.ResolvedAt = DateTime.Now;
                alert.ResolutionType = "Cleared";
                alert.ResetReason = "";
            }

            await db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Alarm cleared MQTT event saved for device ID {WearableDeviceId}", wearableDeviceId);
        }

        private async Task SaveTelemetryAsync(
            FallDetectionDbContext db,
            int wearableDeviceId,
            MqttDeviceMessage message,
            CancellationToken cancellationToken)
        {
            var reading = new SensorReading
            {
                WearableDeviceId = wearableDeviceId,
                RecordedAt = DateTime.Now,

                HeartRateBpm = 0,

                PressureHpa = message.PressureHpa ?? 0,
                AltitudeM = message.Baro?.AltRelSmooth ?? message.Baro?.AltRel ?? message.AltitudeM ?? 0,

                AccelX = message.Imu?.Ax ?? message.AccelX ?? 0,
                AccelY = message.Imu?.Ay ?? message.AccelY ?? 0,
                AccelZ = message.Imu?.Az ?? message.AccelZ ?? 0,

                GyroX = message.Imu?.Gx ?? message.GyroX ?? 0,
                GyroY = message.Imu?.Gy ?? message.GyroY ?? 0,
                GyroZ = message.Imu?.Gz ?? message.GyroZ ?? 0,

                FallDetected = false,
                LedActive = false,
                BuzzerActive = (message.State?.Buzzer ?? 0) == 1,

                MlFallScore = message.Ml?.FallScore ?? message.MlFallScore ?? 0,
                MlTriggered = (message.Ml?.Triggered ?? 0) == 1
            };

            db.SensorReadings.Add(reading);

            await db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Telemetry saved for device ID {WearableDeviceId}. ML score: {MlFallScore}",
                wearableDeviceId,
                reading.MlFallScore);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_mqttClient != null && _mqttClient.IsConnected)
            {
                await _mqttClient.DisconnectAsync(cancellationToken: cancellationToken);
            }

            await base.StopAsync(cancellationToken);
        }

        private class MqttDeviceMessage
        {
            public string DeviceCode { get; set; } = "";
            public string EventType { get; set; } = "";

            public double? PressureHpa { get; set; }
            public double? AltitudeM { get; set; }

            public double? AccelX { get; set; }
            public double? AccelY { get; set; }
            public double? AccelZ { get; set; }

            public double? GyroX { get; set; }
            public double? GyroY { get; set; }
            public double? GyroZ { get; set; }

            public bool? LedActive { get; set; }
            public bool? BuzzerActive { get; set; }

            public double? MlFallScore { get; set; }

            public MqttImuData? Imu { get; set; }
            public MqttBaroData? Baro { get; set; }
            public MqttStateData? State { get; set; }
            public MqttMlData? Ml { get; set; }
        }

        private class MqttImuData
        {
            public double? Ax { get; set; }
            public double? Ay { get; set; }
            public double? Az { get; set; }

            public double? Gx { get; set; }
            public double? Gy { get; set; }
            public double? Gz { get; set; }
        }

        private class MqttBaroData
        {
            public double? AltRel { get; set; }
            public double? AltRelSmooth { get; set; }
            public double? DAlt1s { get; set; }
            public double? DAlt3s { get; set; }
        }

        private class MqttStateData
        {
            public int? IsStill { get; set; }
            public int? FallState { get; set; }
            public int? Alarm { get; set; }
            public int? Buzzer { get; set; }
        }

        private class MqttMlData
        {
            public double? FallScore { get; set; }
            public int? Triggered { get; set; }
        }
    }
}