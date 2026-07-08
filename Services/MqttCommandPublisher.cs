using System.Text;
using System.Text.Json;
using MQTTnet;
using MQTTnet.Protocol;

namespace FallDetectionMonitor.Services;

public class MqttCommandPublisher
{
    private const string MqttBroker = "broker.hivemq.com";
    private const int MqttPort = 1883;
    private const string CommandTopic = "fall-detection-monitor/irfan/commands";

    public async Task SendResetAlarmCommandAsync(string deviceCode)
    {
        var factory = new MqttClientFactory();
        using var client = factory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(MqttBroker, MqttPort)
            .WithClientId($"FallDetectionMonitor_Command_{Guid.NewGuid():N}")
            .Build();

        await client.ConnectAsync(options);

        var payload = JsonSerializer.Serialize(new
        {
            deviceCode = deviceCode,
            command = "reset_alarm"
        });

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(CommandTopic)
            .WithPayload(Encoding.UTF8.GetBytes(payload))
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();

        await client.PublishAsync(message);
        await client.DisconnectAsync();
    }
}