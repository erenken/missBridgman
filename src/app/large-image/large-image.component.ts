import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA} from '@angular/material';
import { LargeImageData } from './largeImageData';

@Component({
  selector: 'app-large-image',
  templateUrl: './large-image.component.html',
  styleUrls: ['./large-image.component.less']
})
export class LargeImageComponent implements OnInit {

  constructor(public dialogRef: MatDialogRef<LargeImageComponent>,
              @Inject(MAT_DIALOG_DATA) public data: LargeImageData) { }

  ngOnInit() {
  }
}
