import { Route } from '@angular/router';

import { KitMasterComponent } from './index';
import { KitDetailComponent } from './index';

export const KitRoutes: Route[] = [
	{
		path: 'kit/cadastro',
		component: KitMasterComponent
	},
    {
		path: 'kit/detail',
		component: KitDetailComponent
	},
];
