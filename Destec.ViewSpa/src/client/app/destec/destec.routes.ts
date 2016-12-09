import { Route } from '@angular/router';

import { KitRoutes } from './kit/index';

import { DestecComponent } from './index';

export const DestecRoutes: Route[] = [
	{
		path: 'destec',
		component: DestecComponent,
		children: [
			...KitRoutes,
		]
	}
];
