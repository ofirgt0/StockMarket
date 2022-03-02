import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MakeADealComponent } from './make-adeal.component';

describe('MakeADealComponent', () => {
  let component: MakeADealComponent;
  let fixture: ComponentFixture<MakeADealComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MakeADealComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MakeADealComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
