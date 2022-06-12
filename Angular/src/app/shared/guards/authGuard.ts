import { Injectable } from '@angular/core';
import { CanLoad, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, UrlSegment, Router, Route } from '@angular/router';
import { Observable } from 'rxjs';
import { SecurityService } from '../../services/security.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanLoad, CanActivate  {
  constructor(private securityService: SecurityService, private router: Router) { }

  canLoad(route: Route, segments: UrlSegment[]): boolean | Observable<boolean> | Promise<boolean> {
    let url: string = route.path!;
    console.log('Url:'+ url);
    if (url =='admin') {
      return false;
    }
    return this.auth();
  }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {

    if (this.securityService.GetRole() === 'admin')
      return true;
    
    this.router.navigate(['/login']);
    return false;
  }

  autorize(): Promise<boolean> {
    return Promise.resolve(false);
  }

  private auth(): Promise<boolean> {
    return this.autorize().then((result: any) => {
      if (!result)
        this.router.navigate(["access-denied"]);
      return Promise.resolve(result);
    });
  }
}
