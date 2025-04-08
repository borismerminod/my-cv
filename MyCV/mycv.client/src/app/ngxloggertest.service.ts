import { Injectable } from '@angular/core';
import { NGXLogger } from 'ngx-logger';

@Injectable({
  providedIn: 'root'
})
export class NGXLoggerServiceTest {
  constructor(private logger: NGXLogger) { }

  logDebug(message: string) {
    this.logger.debug(message);
  }

  logInfo(message: string) {
    this.logger.info(message);
  }

  logWarning(message: string) {
    this.logger.warn(message);
  }

  logError(message: string) {
    this.logger.error(message);
  }
}
