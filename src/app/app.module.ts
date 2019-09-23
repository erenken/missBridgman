import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatButtonModule, MatIconModule, MatToolbarModule, MatMenuModule, MatDividerModule, MatCardModule, MatDialogModule } from '@angular/material';
import { FlexLayoutModule } from '@angular/flex-layout';

import { AppRoutingModule } from './app-routing.module';
import { Browser } from 'protractor';

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
import { LargeImageDialog } from './large-image/large-image.dialog';

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
    LargeImageDialog
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
    MatDialogModule,
    FlexLayoutModule
  ],
  providers: [],
  bootstrap: [RootComponent]
})
export class AppModule { }
