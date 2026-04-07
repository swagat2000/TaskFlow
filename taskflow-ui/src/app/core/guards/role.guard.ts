import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  canActivate(): boolean {
    // Implement role-based access control
    // For now, allow all authenticated users
    return true;
  }
}