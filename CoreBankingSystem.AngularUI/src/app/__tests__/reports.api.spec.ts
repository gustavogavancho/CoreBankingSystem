import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ApiService } from '../shared/api.service';
import { environment } from '../../environments/environment';

describe('Reports integration (ApiService + Report endpoints)', () => {
  let httpMock: HttpTestingController;
  let service: ApiService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ApiService]
    });
    httpMock = TestBed.inject(HttpTestingController);
    service = TestBed.inject(ApiService);
  });

  afterEach(() => httpMock.verify());

  it('builds correct URL for JSON report with identification', () => {
    const params = { identification: 'ID-1001', rangoFechas: '2025-01-01,2025-01-31' };
    service.getReportJson(params).subscribe();

    const req = httpMock.expectOne(`${environment.apiRoot}/report/json?identification=ID-1001&rangoFechas=2025-01-01%2C2025-01-31`);
    expect(req.request.method).toBe('GET');
    req.flush({});
  });

  it('builds correct URL for PDF report with identification', () => {
    const params = { identification: 'ID-1001', rangoFechas: '2025-01-01,2025-01-31' };
    service.getReportPdfBase64(params).subscribe();

    const req = httpMock.expectOne(`${environment.apiRoot}/report/pdf?identification=ID-1001&rangoFechas=2025-01-01%2C2025-01-31`);
    expect(req.request.method).toBe('GET');
    req.flush({ base64: 'ZmFrZQ==' });
  });
});
