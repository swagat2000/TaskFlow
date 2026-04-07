import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  private hubConnection: signalR.HubConnection | undefined;

  startConnection(): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/boardHub')
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR connection started'))
      .catch(err => console.log('Error while starting connection: ' + err));
  }

  addTaskMovedListener(callback: (taskId: string, newStatus: string) => void): void {
    this.hubConnection?.on('TaskMoved', callback);
  }

  addTaskUpdatedListener(callback: (taskId: string) => void): void {
    this.hubConnection?.on('TaskUpdated', callback);
  }

  stopConnection(): void {
    this.hubConnection?.stop();
  }
}