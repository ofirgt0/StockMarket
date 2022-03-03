import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS} from '@angular/common/http';
import { RouterModule, Routes } from '@angular/router';

import { AppComponent } from './app.component';
import { DeallerComponent } from './components/dealler/dealler.component';
import { MenuComponent } from './components/menu/menu.component';
import { HomeComponent } from './components/home/home.component';
import { DeallersComponent } from './components/deallers/deallers.component';
import { AppRoutingModule } from './app-routing.module';
import { AuthComponent } from './auth/auth.component';
import { DeallerCardComponent } from './components/deallers/dealler-card/dealler-card.component';
import { RoundBigNumbersPipe } from './Pipes/RoundBigNumbers';
import { MakeADealComponent } from './components/home/make-adeal/make-adeal.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatSliderModule } from '@angular/material/slider';
import { ChartComponent } from './components/home/chart/chart.component';

const appRoutes: Routes = [
  {path:'' , component: HomeComponent},
  {path:'deallers' , component: DeallersComponent},
  {path:'dealler' , component: DeallerComponent}

  ];

@NgModule({
  declarations: [
    AppComponent,
    
    RoundBigNumbersPipe,
    DeallerComponent,
    MenuComponent,
    HomeComponent,
    DeallersComponent,
    AuthComponent,
    DeallerCardComponent,
    MakeADealComponent,
    ChartComponent,
             
  ],
  imports: [
    BrowserModule,
    FormsModule,
    BrowserAnimationsModule,
    HttpClientModule,
    MatSliderModule,
 
    AppRoutingModule,
    RouterModule.forRoot(appRoutes)
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
