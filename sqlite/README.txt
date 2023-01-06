
 * Database created by importing *.csv file ../world-cities.csv into sqlite - that's why table world-cities has 4 columns.
 
 * There is no single index on that table - uselsess anyway. Each request will yield full scan. We don't care about performance.