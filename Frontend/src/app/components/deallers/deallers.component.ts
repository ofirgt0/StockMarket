import { Component, OnInit } from '@angular/core';
import { BehaviorSubject, Subject, Subscription } from 'rxjs';
import { ServerAccessService } from 'src/app/services/server-access.service';
import { dealler } from 'src/entities.model';


@Component({
  selector: 'app-deallers',
  templateUrl: './deallers.component.html',
  styleUrls: ['./deallers.component.css']
})
export class DeallersComponent implements OnInit {
  deallersSub = new Subject<dealler[]>();
  deallersObs = this.deallersSub.asObservable();
  deallers:dealler[]=[];
  constructor(private accessService:ServerAccessService) { }
  
  ngOnInit(): void {
    this.accessService.getDeallers().subscribe(resDealler=>{this.deallers=resDealler});
    
    console.log(this.deallers);
  }

}
