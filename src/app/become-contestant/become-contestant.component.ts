import { Component, OnInit } from '@angular/core';
import { ContesantFormGroups } from './contestant.form.groups';

@Component({
  selector: 'app-become-contestant',
  templateUrl: './become-contestant.component.html',
  styleUrls: ['./become-contestant.component.less']
})
export class BecomeContestantComponent implements OnInit {

  constructor() { }

  contestantFormGroups = ContesantFormGroups;

  ngOnInit() {
  }

}
