import { AfterViewInit, Component, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { debounceTime, fromEvent, Subscription } from 'rxjs';
import { ajax } from 'rxjs/ajax';

// if term longer in len then 20 chards, don't issue ajax request
const MAX_LEN = 20;

type tMinLen = 1|2|3|4|5;
type tDelay = 200|300|400|500|600|700|800|900|1000;

@Component({
	selector: 'simons-autocomplete',
	template: `
		<ng-content select="[before-label]"></ng-content>
		<label [attr.for]="datalistId" *ngIf="label">{{label}}</label>
		<ng-content select="[after-label]"></ng-content>

		<input [attr.list]="datalistId" #input
		       [name]="datalistId" 
			   [placeholder]="placeholder" 
			   [disabled]="disabled" 
			   autocomplete="off" 					   			   
			   (change)="onChange($event)">

		<datalist [id]="datalistId">			
			<option *ngFor="let option of options;">{{option}}</option>
		</datalist>				

		<ng-content select="[bottom]"></ng-content>
  	`,
	styles: []
})
export class AutocompleteComponent implements OnInit, AfterViewInit, OnDestroy {
	
	@ViewChild('input')
	inputElement!: ElementRef;

	@Input()
	label: string | undefined;

	@Input()
	placeholder = '';

	@Input('datalist-id')
	datalistId!: string;

	@Input()
	options: any[] = [];	

	// Base url for remote data source 
	// Remote endpoint should adhere for convention: 
	//    term - required param passed via query string 
	//    limit - optional numeric param, default is 10, passed via query string
	//    return value - expected to return json array of strings
	// TODO: for real-world component should be refactored
	@Input()
	src!: string;

	// debounce for that much milliseconds - has effect only whith ajax data source
	@Input()
	delay: tDelay = 300;

	@Input()
	disabled = false;

	// only applyes for ajax loading: if term is less in len then minLength, then don't issue request to backend
	@Input()
	minLength: tMinLen  = 2;

	// TODO: validate value between 10 and 1000
	// Applyes for ajax request: how many results to fetch
	@Input()
	maxResults: number = 10;

	// selected value - from input element, *not* from the list!
	// next to this string value there is a valueChange event, 
	// this  will allow who-way binding: [(value)] = ....
	@Input()
	value!: string;	

	@Output()
	valueChange = new EventEmitter<string>();

	@Output()
	ajaxError = new EventEmitter<string>();

	private _subscription: Subscription | undefined;
	private _loading = false;

	get loading() {
		return this._loading;
	}

	constructor() { }
	
	ngOnInit(): void {
		if(!this.datalistId) {
			console.warn('simons-autocomplete not provided with id, assigning generated value ' + this.datalistId);

			let no = Math.floor(Math.random() * 1000000);
			this.datalistId = `simons-autocomplte-${no}`;						
		}		
	}

	
	ngAfterViewInit(): void {		
		// TODO: In order to enable loading results from remote source the 'src' property must be set at the moment of compoent creating
		// If 'src' is empty in the beginnning, and dinamicaly set sometime later, subscribtion will not be created.
		// For real-world component this should be refactored
		if(this.src) {
			const inputElement = this.inputElement.nativeElement;
			const keyups = fromEvent(inputElement, 'keyup');
			const debounced$ = keyups.pipe(debounceTime(this.delay));	// TODO: when 'delay' changes re-create subscription 
	
			let request: Subscription | undefined;			
			
			this._subscription = debounced$.subscribe(_ => { 								
				// if previos request is in progress, we want to cancel it
				// in my backend /Cities endpoint has a CancellationToken param, just for such a case:
				// upon cancellation request server will get a chanse to abort potentially long operation
				if(request) {
					// TODO: I believe this should cancel ther underlying request, not sure...
					request.unsubscribe();		
					request = undefined;
					this._loading = false;			
				}

				const term: string = inputElement.value;

				// issue new request if conditions are met
				if( term.length >= this.minLength && term.length <= MAX_LEN && !this.options.includes(term) ) {
					// TODO: in real-world component we might want to cache results in SessionStorage
					
					const url = new URL(this.src);
					url.searchParams.append("term", term);
					url.searchParams.append("limit", this.maxResults.toString());

					// now go!
					request = ajax
						.getJSON(url.toString())
						.subscribe({
							next: (v) => this.options = v as string[],
							complete: () => this._loading = false,
							error: err => { 
								console.error(err);
								this.ajaxError.emit(err.message);
								this._loading = false;
							}
						});
				}
			});		
		}		
	}


	ngOnDestroy(): void {
		if(this._subscription) {
			this._subscription.unsubscribe();
		}
	}


	onChange(event: Event) {
		let input = event.target as HTMLInputElement;		
		this.valueChange.emit(input.value);
	}	
}
