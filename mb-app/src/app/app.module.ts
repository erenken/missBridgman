import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatDividerModule } from '@angular/material/divider';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatDialogModule } from '@angular/material/dialog';

import { AppRoutingModule } from './app-routing.module';

import { HomeComponent } from './home/home.component';
import { RootComponent } from './root/root.component';
import { MenuComponent } from './menu/menu.component';
import { FooterComponent } from './footer/footer.component';
import { WhoweareComponent } from './whoweare/whoweare.component';
import { SponsorsComponent } from './sponsors/sponsors.component';
import { PhotosComponent } from './photos/photos.component';
import { InfoComponent } from './info/info.component';
import { BecomeContestantComponent } from './become-contestant/become-contestant.component';
import { CurrentCourtComponent } from './current-court/current-court.component';
import { FormsComponent } from './forms/forms/forms.component';
import { TicketsComponent } from './tickets/tickets.component';

@NgModule({
  declarations: [
    HomeComponent,
    RootComponent,
    MenuComponent,
    FooterComponent,
    WhoweareComponent,
    SponsorsComponent,
    PhotosComponent,
    InfoComponent,
    BecomeContestantComponent,
    CurrentCourtComponent,
    FormsComponent,
    TicketsComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MatButtonModule,
    MatIconModule,
    MatToolbarModule,
    MatMenuModule,
    MatDividerModule,
    MatCardModule,
    MatDialogModule
  ],
  providers: [],
  bootstrap: [RootComponent]
})
export class AppModule { }