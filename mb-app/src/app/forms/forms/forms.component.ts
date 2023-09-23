import { Component, OnInit } from '@angular/core';
import { FormItems } from './forms.items';

@Component({
  selector: 'app-forms',
  templateUrl: './forms.component.html',
  styleUrls: ['./forms.component.less']
})
export class FormsComponent implements OnInit {

  constructor() { }

  formItems = FormItems;
  
  ngOnInit(): void {
  }

}
