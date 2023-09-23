import { Component, OnInit } from '@angular/core';
import { ApiService } from '../service/apiService';
import { Observable } from 'rxjs';
import { Folder } from '../service/apiInterfaces';

@Component({
  selector: 'app-forms',
  templateUrl: './documents.component.html',
  styleUrls: ['./documents.component.less']
})
export class DocumentsComponent implements OnInit {

  constructor(private apiService: ApiService) { }

  public documents$: Observable<Folder> = new Observable<Folder>();
  
  ngOnInit(): void {
    this.documents$ = this.apiService.getDocuments();
  }
}
