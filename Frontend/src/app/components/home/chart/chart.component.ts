import { Component, OnInit } from '@angular/core';
import Chart from 'chart.js/auto';
import { BackendAccessService } from 'src/app/services/BackendAccess.service';
import { ChartManagerService } from 'src/app/services/chart-manager.service';
import { DataContainerService } from 'src/app/services/data-container.service';
import { stock } from 'src/entities.model';
type chartParams = { data: any; labels: any };
@Component({
  selector: 'app-chart',
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.css'],
})
export class ChartComponent implements OnInit {
  stocks: stock[] = [];
  stocksToShow = [];
  stockNameToShow: string = 'Apple';
  myChart: any;
  constructor(
    private accessService: BackendAccessService,
    private dataContainer: DataContainerService,
    private chartService: ChartManagerService
  ) {}

  async ngOnInit(): Promise<void> {
    this.chartService.getStockNameValue().subscribe((updatedStockName) => {
      this.stockNameToShow = updatedStockName;
      this.chartHandlerAsync(this.stockNameToShow);
    });
  }
  chartDataHandler(stock: stock): chartParams {
    const data = [];
    const labels = [];
    var price = stock.currentPrice;
    for (var i = stock.percentageDifference.length - 1; i > 0; i--) {
      labels[stock.percentageDifference.length - 1 - i] =
        'day ' + (stock.percentageDifference.length - i);
      data[stock.percentageDifference.length - 1 - i] =
        (stock.percentageDifference[i] * price) / 100 + price;
      price = data[stock.percentageDifference.length - 1 - i];
    }
    return { data, labels };
  }

  showStockChart(stockName: string) {
    var colorsSet = [
      '#1F2041',
      '#4B3F72',
      '#FFC857',
      '#119DA4',
      '#19647E',
      '#588157',
      '#3A5A40',
      '#C03221',
      '#f7f7ff',
      '#5AD8CE',
      '#3F826D',
    ];
    var stock = this.stocks.find((stock) => stock.name == stockName)!;
    const stockData = {
      labels: this.chartDataHandler(stock).labels,
      datasets: [
        {
          label: stockName,
          data: this.chartDataHandler(stock).data,
          fill: false,
          borderColor: colorsSet[Math.round(stock.currentPrice % 10)],
          tension: 0.1,
        },
      ],
    };
    return stockData;
  }
  async chartHandlerAsync(stockToShow: string) {
    (await this.dataContainer.getStocksAsync()).subscribe((resStocks) => {
      this.stocks = resStocks;

      this.myChart = new Chart(this.stockNameToShow, {
        type: 'line',
        data: this.showStockChart(stockToShow),
        options: {
          scales: {
            y: {
              beginAtZero: false,
            },
          },
        },
      });
    });
  }
}
