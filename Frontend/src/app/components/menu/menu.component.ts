import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { BackendAccessService } from 'src/app/services/BackendAccess.service';
@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css'],
})
export class MenuComponent implements OnInit {
  isAuth = false;
  deallerName = '';

  constructor(private backendAccess: BackendAccessService,private toastr: ToastrService) {}

  async ngOnInit(): Promise<void> {
    this.backendAccess.setAuthValue(this.isAuth);
    this.backendAccess.getAuthValue().subscribe(data=>this.isAuth==data)
  }

  Logout() {
    this.isAuth = false;
    this.backendAccess.setAuthValue(this.isAuth);
  }
  
  async deallerLoginAsync(id: any) {
    const a = await this.backendAccess.onLoginAsync(id);    
    a.subscribe((data) => {
      this.backendAccess.getAuthValue().subscribe(isAuthRet=>this.isAuth=isAuthRet);
      if(data==null){
        this.toastr.error('Wrong ID! Try again ');
      }
      else{
        this.deallerName = data.name;
        this.toastr.success(this.deallerName + ' Welcom back!');
        this.backendAccess.updateAuthDeallerName(this.deallerName);
        this.backendAccess.setAuthValue(this.isAuth);
      }
    });
  }
  getOfflineDealsCounter(deallerName:string)
  {

  }
}
