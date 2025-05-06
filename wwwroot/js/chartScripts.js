window.drawLineChart = () => {
    const ctx = document.getElementById('lineChart').getContext('2d');

    new Chart(ctx, {
        type: 'line',
        data: {
            labels: [0, 1, 2, 3, 4, 5],
            datasets: [{
                label: 'Sample Data',
                data: [5, 10, 8, 15, 12, 18],
                borderColor: 'rgba(75, 192, 192, 1)',
                borderWidth: 2,
                fill: false
            }]
        },
        options: {
            responsive: true,
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
};
