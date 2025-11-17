import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-emails-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './emails-list.component.html',
  styleUrl: './emails-list.component.scss'
})
export class EmailsListComponent {
  // Por enquanto, apenas um placeholder
  // Depois vamos implementar a lista de e-mails
}

