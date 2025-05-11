window.drawLineChart = (labels, data) => {
    const ctxLine = document.getElementById('lineChart').getContext('2d');

    if (window.lineInstance) {
        window.lineInstance.destroy();
    }

    window.lineInstance = new Chart(ctxLine, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Net Cash Flow',
                data: data,
                borderColor: 'rgba(75, 192, 192, 1)',
                borderWidth: 2,
                fill: false
            }]
        },
        options: {
            responsive: true,
            scales: {
                y: { beginAtZero: true }
            }
        }
    });
};

window.drawPieChart = (labels, data) => {
    const ctxPie = document.getElementById('pieChart').getContext('2d');

    if (window.pieInstance) {
        window.pieInstance.destroy();
    }

    window.pieInstance = new Chart(ctxPie, {
        type: 'pie',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: [
                    'rgba(75, 192, 192, 0.6)',
                    'rgba(255, 99, 132, 0.6)',
                    'rgba(255, 206, 86, 0.6)',
                    'rgba(102, 204, 255, 0.6)'
                ],
                borderColor: [
                    'rgba(75, 192, 192, 1)',
                    'rgba(255, 99, 132, 1)',
                    'rgba(255, 206, 86, 1)',
                    'rgba(102, 204, 255, 1)'
                ],
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'right'
                }
            }
        }
    });
};

window.drawBarChart = (labels, data) => {
    const ctxBar = document.getElementById('barChart').getContext('2d');

    if (window.barInstance) {
        window.barInstance.destroy();
    }

    window.barInstance = new Chart(ctxBar, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Activity',
                data: data,
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

window.drawUserLineChart = (labels, withdrawData, depositData, outgoingTransfer, incomingTransfer, loanPayment, netBalance) => {
    const ctxUserLine = document.getElementById('UserLineChart').getContext('2d');

    if (window.lineInstance) {
        window.lineInstance.destroy();
    }

    window.lineInstance = new Chart(ctxUserLine, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [
                {
                    label: 'Net Balance',
                    data: netBalance,
                    backgroundColor: 'rgba(211, 211, 211, 0.6)',
                    borderWidth: 2,
                    fill: false
                },
                {
                    label: 'Withdraw',
                    data: withdrawData,
                    borderColor: 'rgba(75, 192, 192, 0.6)',
                    borderWidth: 2,
                    fill: false
                },
                {
                    label: 'Deposit',
                    data: depositData,
                    borderColor: 'rgba(255, 99, 132, 0.6)',
                    borderWidth: 2,
                    fill: false
                },
                {
                    label: 'Outgoing Transfer',
                    data: outgoingTransfer,
                    borderColor: 'rgba(76, 206, 86, 0.6)',
                    borderWidth: 2,
                    fill: false
                },
                {
                    label: 'Incoming Transfer',
                    data: incomingTransfer,
                    borderColor: 'rgba(102, 204, 255, 0.6)',
                    borderWidth: 2,
                    fill: false
                },
                {
                    label: 'Loan Payment',
                    data: loanPayment,
                    borderColor: 'rgba(255, 206, 86, 0.6)',
                    borderWidth: 2,
                    fill: false
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'right',
                    align: 'start'
                }
            },
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
};
window.drawDashboardBarChart = (labels, data1, data2, data3, data4) => {
    const ctxBar = document.getElementById('barChart').getContext('2d');

    if (window.barInstance) {
        window.barInstance.destroy();
    }

    window.barInstance = new Chart(ctxBar, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [
                {
                    label: 'Withdraw',
                    data: data1,
                    backgroundColor: 'rgba(255, 99, 132, 0.6)',
                    borderColor: 'rgba(255, 99, 132, 1)',
                    borderWidth: 1
                },
                {
                    label: 'Deposit',
                    data: data2,
                    backgroundColor: 'rgba(75, 192, 192, 0.6)',
                    borderColor: 'rgba(75, 192, 192, 1)',
                    borderWidth: 1
                },
                {
                    label: 'Transfer',
                    data: data3,
                    backgroundColor: 'rgba(255, 206, 86, 0.6)',
                    borderColor: 'rgba(255, 206, 86, 1)',
                    borderWidth: 1
                },
                {
                    label: 'Loan Payment',
                    data: data4,
                    backgroundColor: 'rgba(102, 204, 255, 0.6)',
                    backgroundColor: 'rgba(51, 204, 255, 1)',
                    borderWidth: 1
                }
            ]
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
