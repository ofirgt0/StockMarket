import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CommonModule } from '@angular/common';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrModule } from 'ngx-toastr';
import { BrowserModule } from '@angular/platform-browser';
import { NgbToastModule } from  'ngb-toast';
const routes: Routes = [];

@NgModule({
  imports: [RouterModule.forRoot(routes),CommonModule,BrowserModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot()],
    
  exports: [RouterModule]
})
export class AppRoutingModule { }
