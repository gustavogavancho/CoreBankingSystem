import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-search-bar',
  template: `
    <div class="search-bar">
      <input
        type="text"
        [placeholder]="placeholder || 'Buscar...'"
        [(ngModel)]="term"
        (keyup.enter)="onSearch()"
      />
      <button type="button" (click)="onSearch()">Buscar</button>
    </div>
  `,
  styles: [`
    .search-bar{display:flex;gap:.5rem;align-items:center;margin:.5rem 0}
    input{flex:1;padding:.5rem;border:1px solid #ccc;border-radius:4px}
    button{padding:.5rem .75rem;border:1px solid #333;background:#fff;cursor:pointer;border-radius:4px}
    button:hover{background:#f5f5f5}
  `]
})
export class SearchBarComponent {
  @Input() placeholder = '';
  @Output() search = new EventEmitter<string>();
  term = '';

  onSearch(){
    this.search.emit(this.term.trim());
  }
}
