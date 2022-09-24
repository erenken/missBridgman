import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { CurrentCourtCollection } from './current-court';
import { LargeImageComponent } from '../large-image/large-image.component';

@Component({
  selector: 'app-current-court',
  templateUrl: './current-court.component.html',
  styleUrls: ['./current-court.component.less']
})
export class CurrentCourtComponent implements OnInit {

  constructor(public dialog: MatDialog) { }

  currentCourt = CurrentCourtCollection;

  ngOnInit() {
  }

  openDialog(e: any): void {
    const dialogRef = this.dialog.open(LargeImageComponent, {
      autoFocus: true,
      hasBackdrop: true,
      data: { src: e.currentTarget.currentSrc, alt: e.currentTarget.alt }
    });
  }
}
