import { Component, OnInit } from '@angular/core';
import { DeallerManageService } from 'src/app/services/dealler-manage.service';
import { CommonModule } from '@angular/common';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrService } from 'ngx-toastr';



@Component({
  selector: 'app-dealler',
  templateUrl: './dealler.component.html',
  styleUrls: ['./dealler.component.css']
})
export class DeallerComponent implements OnInit {

  constructor(private deallerServ:DeallerManageService) { }

  ngOnInit(): void {
    
  }

}
