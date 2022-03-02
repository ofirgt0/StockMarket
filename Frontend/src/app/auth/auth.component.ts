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
    (await this.aServ.onLogin(id)).subscribe(data=>{this.isAuth=data!=null});
    console.log(this.isAuth)
    this.authProblem=!this.isAuth; 

    this.aServ.updateAuthStatus(this.isAuth);
  }

}
