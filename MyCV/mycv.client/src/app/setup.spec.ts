import { TestBed } from '@angular/core/testing';
import { NGXLoggerServiceTest } from './ngxloggertest.service';
import { NGXLogger } from 'ngx-logger';
import { LoggerTestingModule } from 'ngx-logger/testing';


describe('LoggerService', () => {
  let loggerService: NGXLoggerServiceTest;
  let ngxLogger: NGXLogger;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [LoggerTestingModule],
      providers: [
        NGXLoggerServiceTest
      ]
    });

    ngxLogger = TestBed.inject(NGXLogger);
    console.log(ngxLogger)
    loggerService = TestBed.inject(NGXLoggerServiceTest);

  });

  it('should call NGXLogger.debug()', () => {

    const spy = spyOn(ngxLogger, 'debug');
    loggerService.logDebug('Test debug');
    expect(spy).toHaveBeenCalledWith('Test debug');
  });

  it('should call NGXLogger.info()', () => {
    spyOn(ngxLogger, 'info');

    loggerService.logInfo('Test info');

    expect(ngxLogger.info).toHaveBeenCalledWith('Test info');
  });

  it('should call NGXLogger.warn()', () => {
    spyOn(ngxLogger, 'warn');

    loggerService.logWarning('Test warning');

    expect(ngxLogger.warn).toHaveBeenCalledWith('Test warning');
  });

  it('should call NGXLogger.error()', () => {
    spyOn(ngxLogger, 'error');

    loggerService.logError('Test error');

    expect(ngxLogger.error).toHaveBeenCalledWith('Test error');
  });
});

