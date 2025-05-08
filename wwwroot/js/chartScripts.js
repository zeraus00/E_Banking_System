window.drawLineChart = () => {
    const ctxLine = document.getElementById('lineChart').getContext('2d');

    new Chart(ctxLine, {
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

window.drawPieChart = () => {
    const ctxPie = document.getElementById('pieChart').getContext('2d');

    new Chart(ctxPie, {
        type: 'pie',
        data: {
            labels: ['Withdrawals', 'Deposits', 'Transfers', 'Loans'],
            datasets: [{
                label: 'Transaction Type Distribution',
                data: [25, 40, 25, 10],
                backgroundColor: [
                    'rgba(75, 192, 192, 0.6)',
                    'rgba(255, 99, 132, 0.6)',
                    'rgba(255, 206, 86, 0.6)', 
                    'rgba(200, 100, 0, 0.6)'
                ],
                borderColor: [
                    'rgba(75, 192, 192, 1)',
                    'rgba(255, 99, 132, 1)',
                    'rgba(255, 206, 86, 1)', 
                    'rgba(200, 100, 0, 0.6)'
                ],
                borderWidth: 1
            }]
        },
        options: {
            responsive: true
        }
    });
};

window.drawBarChart = () => {
    const ctxBar = document.getElementById('barChart').getContext('2d');
    const barFilter = document.getElementById('barChartFilter').value;

    let _label = [];
    let _data = [];
    let barInstance;

    switch (barFilter) {
        case 'hourly':
            _label = ['8AM', '9AM', '10AM', '11AM', '12PM'];
            _data = [10, 15, 8, 20, 25];
            break;
        case 'daily':
            _label = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri'];
            _data = [120, 150, 130, 170, 160];
            break;
        case 'weekly':
            _label = ['Week 1', 'Week 2', 'Week 3', 'Week 4'];
            _data = [400, 450, 420, 480];
            break;
    }

    if (barInstance) {
        barInstance.destroy();
    }

    barInstance = new Chart(ctxBar, {
        type: 'bar',
        data: {
            labels: _label,
            datasets: [{
                data: _data,
                backgroundColor: 'rgba(54, 162, 235, 0.6)',
                borderColor: 'rgba(54, 162, 235, 1)',
                borderWidth: 1
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
