import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  constructor(private http: HttpClient) { }

  private Url = environment.apiUrl + "/v1/CommentApi";
  httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };

  SendComment(name: string, email: string, description: string, productId: string, parentCommentId?: number) {
    return this.http.post(`${this.Url}?name=${name}&email=${email}&description=${description}&productId=${productId}&parentCommentId=${parentCommentId}`, this.httpOptions);
  }

  LikeOrDisLike(commentId: number, browserId: string, isLiked: boolean): Observable<any> {
    return this.http.get<any>(`${this.Url}?commentId=${commentId}&browserId=${browserId}&isLiked=${isLiked}`, this.httpOptions);
  }
}
