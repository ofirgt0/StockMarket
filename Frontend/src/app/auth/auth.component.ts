import { Component, OnInit } from '@angular/core';
import { ServerAccessService } from '../services/server-access.service';

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.css']
})
export class AuthComponent implements OnInit {
  isAuth=false;
  authProblem=false;
  deallerName=""
  constructor(private aServ:ServerAccessService) { }
  
  async ngOnInit(): Promise<void> {
    this.aServ.updateAuthStatus(await this.isAuth);
  }
  Logout(){
    this.isAuth=false;
    this.aServ.updateAuthStatus(this.isAuth);
  }
  async deallerLogin(id:any)
  {
    console.log(id);
    (await this.aServ.onLogin(id)).subscribe(data=>{
      this.isAuth=data!=null;
      this.deallerName=data.name;
      if(this.isAuth){
        this.aServ.updateAuthDeallerName(this.deallerName);
        this.aServ.updateAuthStatus(this.isAuth);
      }
      else
        this.authProblem=!this.isAuth; 
    });
    
  }

}
