import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA} from '@angular/material';
import { LargeImageData } from './largeImageData';

@Component({
  selector: 'app-large-image',
  templateUrl: './large-image.dialog.html',
  styleUrls: ['./large-image.dialog.less']
})
export class LargeImageDialog implements OnInit {

  constructor(public dialogRef: MatDialogRef<LargeImageDialog>,
    @Inject(MAT_DIALOG_DATA) public data: LargeImageData) { }

  ngOnInit() {
  }

  onCloseClick(): void {
    this.dialogRef.close();
  }
}
