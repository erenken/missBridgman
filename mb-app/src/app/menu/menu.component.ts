import { Component, OnInit } from '@angular/core';
import { MenuItems } from './menu.items';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.less']
})
export class MenuComponent implements OnInit {

  constructor() { }

  menuItems = MenuItems;

  ngOnInit() {
  }
}
