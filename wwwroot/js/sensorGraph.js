window.sensorGraph = {
    charts: {},

    render: function (data) {
        if (!window.Chart) {
            console.error("Chart.js is not loaded.");
            return;
        }

        this.renderLineChart("accChart", "Accelerometer", data.labels, [
            { label: "Accel X", data: data.accelX },
            { label: "Accel Y", data: data.accelY },
            { label: "Accel Z", data: data.accelZ },
            { label: "Accel Mag", data: data.accelMag }
        ]);

        this.renderLineChart("gyroChart", "Gyroscope", data.labels, [
            { label: "Gyro X", data: data.gyroX },
            { label: "Gyro Y", data: data.gyroY },
            { label: "Gyro Z", data: data.gyroZ },
            { label: "Gyro Mag", data: data.gyroMag }
        ]);

        this.renderLineChart("baroChart", "Barometer", data.labels, [
            { label: "Pressure hPa", data: data.pressure },
            { label: "Altitude m", data: data.altitude }
        ]);

        this.renderLineChart("mlChart", "TinyML Fall Score", data.labels, [
            { label: "ML Fall Score", data: data.mlFallScore }
        ]);
    },

    renderLineChart: function (canvasId, title, labels, datasets) {
        const canvas = document.getElementById(canvasId);

        if (!canvas) {
            return;
        }

        if (this.charts[canvasId]) {
            this.charts[canvasId].destroy();
        }

        this.charts[canvasId] = new Chart(canvas, {
            type: "line",
            data: {
                labels: labels,
                datasets: datasets.map((dataset) => {
                    const lower = dataset.label.toLowerCase();

                    let color = "#111827"; // default black

                    if (lower.includes(" x")) {
                        color = "#2563eb"; // X = blue
                    }
                    else if (lower.includes(" y")) {
                        color = "#dc2626"; // Y = red
                    }
                    else if (lower.includes(" z")) {
                        color = "#eab308"; // Z = yellow
                    }
                    else if (lower.includes("mag")) {
                        color = "#111827"; // magnitude = black
                    }
                    else if (lower.includes("pressure")) {
                        color = "#2563eb"; // pressure = blue
                    }
                    else if (lower.includes("altitude")) {
                        color = "#dc2626"; // altitude = red
                    }

                    return {
                        label: dataset.label,
                        data: dataset.data,
                        borderColor: color,
                        backgroundColor: color,
                        borderWidth: 2,
                        tension: 0.25,
                        pointRadius: 1
                    };
                })
            },
            options: {
                responsive: true,
                animation: false,
                maintainAspectRatio: false,
                plugins: {
                    title: {
                        display: true,
                        text: title
                    },
                    legend: {
                        display: true
                    }
                },
                scales: {
                    x: {
                        ticks: {
                            maxTicksLimit: 10
                        }
                    }
                }
            }
        });
    }
};