create database PharmaciesController;
go;

use PharmaciesController;
go;

create table goods (
	good_id int not null identity(0,1) primary key,
	title varchar(256) not null
)

create table pharmacies (
	pharmacy_id int not null identity(0,1) primary key,
	title varchar(256) not null,
	addr varchar(256) not null,
	phone varchar(10) not null
)

create table storages (
	storage_id int not null identity(0,1) primary key,
	pharmacy_id int not null foreign key references pharmacies(pharmacy_id) on update cascade on delete cascade,
	title varchar(256)
)

create table butches (
	butch_id int not null identity(0,1) primary key,
	good_id int not null foreign key references goods(good_id) on update cascade on delete cascade,
	storage_id int not null foreign key references storages(storage_id) on update cascade on delete cascade,
	amount int not null
)

-- подключить через Server Explorer используя LINQ to SQL