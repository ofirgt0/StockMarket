import { Component, OnInit } from '@angular/core';
import { ServerAccessService } from 'src/app/services/server-access.service';
@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent implements OnInit {

  isAuth=false;
  authProblem=false;
  deallerName=""
  
  constructor(private aServ:ServerAccessService) { }
  
  async ngOnInit(): Promise<void> {
    this.aServ.setAuthValue(this.isAuth);
  }
  Logout(){
    this.isAuth=false;
    this.aServ.setAuthValue(this.isAuth);
  }
  async deallerLogin(id:any)
  {
    
    (await this.aServ.onLogin(id)).subscribe(data=>{
      this.isAuth=data!=null;
      this.deallerName=data.name;
      if(this.isAuth){
        this.aServ.updateAuthDeallerName(this.deallerName);
        console.log("isAuth from menu - "+this.isAuth);
        this.aServ.setAuthValue(this.isAuth);
      }
      else
        this.authProblem=!this.isAuth; 
    });
    
  }
  
 
}
