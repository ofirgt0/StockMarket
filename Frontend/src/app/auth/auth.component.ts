import { Component, OnInit } from '@angular/core';
import { ServerAccessService } from '../services/server-access.service';

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.css']
})
export class AuthComponent implements OnInit {
  isAuth=false;
  authProb=false;
  constructor(private aServ:ServerAccessService) { }
  
  async ngOnInit(): Promise<void> {
    this.aServ.updateAuthStatus(await this.isAuth);
  }
  Logout(){
    this.isAuth=false;
    this.aServ.updateAuthStatus(this.isAuth);
  }
  deallerLogin(id:any)
  {
    console.log(id)
    this.aServ.onLogin(id).then(res=>this.isAuth);
    console.log(this.isAuth)
    this.authProb=!this.isAuth; 

    this.aServ.updateAuthStatus(this.isAuth);
  }

}
