import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import { AboutComponent } from './other/about/about.component';

@NgModule({
  imports: [CommonModule, RouterModule],
  declarations: [HeaderComponent, FooterComponent, AboutComponent],
  exports: [HeaderComponent, FooterComponent],
})
export class UIModule {}
