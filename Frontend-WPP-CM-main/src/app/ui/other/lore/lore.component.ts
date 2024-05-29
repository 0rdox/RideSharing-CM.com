import { Component, OnInit } from '@angular/core';
import { OtherService } from '../other.service';
import { Observable } from 'rxjs';
@Component({
  selector: 'app-Lore',
  templateUrl: './lore.component.html',
  styleUrl: './lore.component.css',
})
export class LoreComponent {
  //   lore?: string;
  //   constructor(private readonly otherService: OtherService) {}
  //   ngOnInit(): void {
  //     this.otherService.getLore().subscribe((lore: string) => {
  //       this.lore = lore;
  //     });
  //   }
}
