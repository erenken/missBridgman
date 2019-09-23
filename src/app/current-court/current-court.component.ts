import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { LargeImageDialog } from '../large-image/large-image.dialog';

@Component({
  selector: 'app-current-court',
  templateUrl: './current-court.component.html',
  styleUrls: ['./current-court.component.less']
})
export class CurrentCourtComponent implements OnInit {

  constructor(public dialog: MatDialog) { }

  ngOnInit() {
  }

  openDialog(e: any): void {
    const dialogRef = this.dialog.open(LargeImageDialog, {
      data: { src: e.currentTarget.currentSrc, alt: e.currentTarget.alt }
    });
  }
}
