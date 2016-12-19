import { Route } from '@angular/router';

import { PedidoMasterComponent } from './index';
import { PedidoDetailComponent } from './index';

export const PedidoRoutes: Route[] = [
	{
		path: 'pedido/cadastro',
		component: PedidoMasterComponent
	},
    {
		path: 'pedido/detail',
		component: PedidoDetailComponent
	},
];
