import * as gulp from 'gulp';
import { writeFileSync } from 'fs';

import Config from '../../config';

/**
 * Executes the build process, generating the manifest file using `angular2-service-worker`.
 */
export = () => {
    return writeFileSync(Config.APP_DEST + '/Staticfile', 'pushstate: enabled');
};
