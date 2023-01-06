import { Component } from '@angular/core';


@Component({
	selector: 'app-root',
	templateUrl: './app.component.html',
	styleUrls: ['./app.component.css']
})
export class AppComponent {
	title = 'demo';

	demo1 = {
		label: 'Something:',
		placeholder: "Start typing...",
		text: "quark\nsquark\nelectron\nselectron\nneutrino\nsneutrino\ngraviton\ngravitino\ngluon\ngluino\nphoton\nphotino\nHiggs\nHiggsino",
		disabled: false,
		value: '',
		lines: function() {
			return this.text.split("\n").map(w => w.trim());
		}
	};

	demo2 = {
		label: 'Something else:',
		placeholder: "Type here...",		
		lines: ['quark','squark','electron','selectron','neutrino','sneutrino','graviton','gravitino','gluon','gluino','photon','photino','Higgs','Higgsino']
	};

	demo3 = {
		limit: 10
	}
}