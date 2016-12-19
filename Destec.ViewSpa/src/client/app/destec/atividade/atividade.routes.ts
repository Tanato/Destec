import { Route } from '@angular/router';

import { AtividadeMasterComponent } from './index';
//import { AtividadeDetailComponent } from './index';

export const AtividadeRoutes: Route[] = [
	{
		path: 'atividade/master',
		component: AtividadeMasterComponent
	},
    // {
	// 	path: 'atividade/detail',
	// 	component: AtividadeDetailComponent
	// },
];
