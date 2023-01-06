import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AutocompleteComponent } from './autocomplete.component';

describe('AutocompleteComponent', () => {
	let component: AutocompleteComponent;
	let fixture: ComponentFixture<AutocompleteComponent>;

	beforeEach(async () => {
		await TestBed.configureTestingModule({
			declarations: [AutocompleteComponent]
		})
		.compileComponents();

		fixture = TestBed.createComponent(AutocompleteComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});

	it('should be auto-filled datalistId', () => {
		expect(component.datalistId).toBeTruthy();
		console.log(component.datalistId)
	});


	it('should have defaults properly set', () => {		
		expect(component.label).toBeUndefined();
		expect(component.placeholder).toEqual('');
		expect(component.options).toEqual([]);
		expect(component.src).toBeFalsy();
		expect(component.disabled).toBeFalse();
		expect(component.minLength).toEqual(2);
		expect(component.maxResults).toEqual(10);
		expect(component.value).toBeFalsy();
	});	
});
