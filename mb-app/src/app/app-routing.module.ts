import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { WhoweareComponent } from './whoweare/whoweare.component';
import { SponsorsComponent } from './sponsors/sponsors.component';
import { PhotosComponent } from './photos/photos.component';
import { InfoComponent } from './info/info.component';
import { DocumentsComponent } from './documents/documents.component';



const routes: Routes = [
  { path: 'home', component: HomeComponent },
  { path: 'whoweare', component: WhoweareComponent },
  { path: 'sponsors', component: SponsorsComponent },
  { path: 'photos', component: PhotosComponent },
  { path: 'info', component: InfoComponent },
  { path: 'documents', component: DocumentsComponent },
  { path: '', redirectTo: '/home', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
