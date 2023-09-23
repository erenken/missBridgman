import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "src/environments/environment";
import { Folder } from "./apiInterfaces";

@Injectable()
export class ApiService {
    constructor(private http: HttpClient) {}

    getDocuments(): Observable<Folder> {
        return this.http.get<Folder>(environment.getDocumentsApi);
    }

    getImages(year?: string): Observable<Folder> {
        return this.http.get<Folder>(environment.getImagesApi.replace('{year}', year || ''));
    }
}