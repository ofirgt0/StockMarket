import { Component, OnInit, TemplateRef } from '@angular/core';
import { BackendAccessService } from 'src/app/services/BackendAccess.service';
import { stock, Offer, ActionPerformedType } from 'src/entities.model';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { DataContainerService } from 'src/app/services/data-container.service';

@Component({
  selector: 'app-make-adeal',
  templateUrl: './make-adeal.component.html',
  styleUrls: ['./make-adeal.component.css'],
})
export class MakeADealComponent implements OnInit {
  stocks: stock[] = [];
  newOffer!: Offer;
  constructor(
    private accessService: BackendAccessService,
    private toastr: ToastrService,
    private dataContainer: DataContainerService
  ) {}

  async ngOnInit(): Promise<void> {
    (await this.dataContainer.getStocksAsync()).subscribe((resStocks) => {
      this.stocks = resStocks;
    });
  }
  formatLabel(value: number) {
    if (value >= 1000) return Math.round(value / 1000);
    return value;
  }
  formatLabelK(value: number) {
    if (value >= 1000) return Math.round(value / 1000) + 'K';
    return value;
  }
  makeToast(res: any) {
    if (res['action'] == ActionPerformedType.WasFullyExecuted)
      this.toastr.success('The transaction was fully executed');

    if (res['action'] == ActionPerformedType.WasPartiallyExecuted)
      this.toastr.success(
        'The transaction Was Partially Executed, we upload a new offer with ' +
          res['QuantityRemaining'] +
          "stock's"
      );

    if (res['action'] == ActionPerformedType.WasNotExecutedNewOfferHasUploaded)
      this.toastr.success(
        'The transaction was not Executed because there is no relevant offer for you. The deal was raised as a new offer'
      );

    if (res['action'] == ActionPerformedType.WasNotExecutedOfferNotPossible)
      this.toastr.error(
        'The deal was not executed! Please fix the data and try again'
      );
  }

  async onSubmitAsync(form: NgForm) {
    (await this.accessService.makeADealAsync(form))
      .toPromise()
      .then((data) => this.makeToast(data));
  }
}
