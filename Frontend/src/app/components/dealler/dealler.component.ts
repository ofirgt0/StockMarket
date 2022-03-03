import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrService } from 'ngx-toastr';
import { ServerAccessService } from 'src/app/services/server-access.service';



@Component({
  selector: 'app-dealler',
  templateUrl: './dealler.component.html',
  styleUrls: ['./dealler.component.css']
})
export class DeallerComponent implements OnInit {

  constructor(private accessService:ServerAccessService) { 
    accessService
  }


  ngOnInit(): void {
    
  }

}
