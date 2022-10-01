import { Component, OnInit } from '@angular/core';
import { TicketItems } from './ticket.items';

@Component({
  selector: 'app-tickets',
  templateUrl: './tickets.component.html',
  styleUrls: ['./tickets.component.less']
})
export class TicketsComponent implements OnInit {

  constructor() { }

  ticketItems = TicketItems;

  ngOnInit(): void {
  }

}
