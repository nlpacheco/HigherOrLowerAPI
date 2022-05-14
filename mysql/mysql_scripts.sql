SET @@global.time_zone  = '-03:00';
SET @@session.time_zone = '-03:00';
SET @@GLOBAL.sql_mode   ='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION'; 
SET @@SESSION.sql_mode  ='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION'; 

DROP TABLE IF EXISTS player_moves;
DROP TABLE IF EXISTS game_players;
DROP TABLE IF EXISTS games;


CREATE TABLE IF NOT EXISTS games (
	id 					int UNSIGNED 		NOT NULL AUTO_INCREMENT,
	cards				varchar(2000)		NOT NULL,
	player_count		int					NOT NULL,
	current_round		int					NOT NULL,
	created_at 			TIMESTAMP 			NOT NULL DEFAULT CURRENT_TIMESTAMP,
	updated_at 			TIMESTAMP 			NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT		Pk_games PRIMARY KEY (id)
);


CREATE TABLE IF NOT EXISTS game_players (
	id 					int UNSIGNED 		NOT NULL AUTO_INCREMENT,
	player_name			varchar(100)		NOT NULL,
	wins				int					NOT NULL,
	loses				int					NOT NULL,
	round_position		int					NOT NULL,
	game_id 			int UNSIGNED 		NOT NULL,
	created_at 			TIMESTAMP 			NOT NULL DEFAULT CURRENT_TIMESTAMP,
	updated_at 			TIMESTAMP 			NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT		Pk_game_players 		PRIMARY KEY (id),
	FOREIGN KEY 	Fk_game_players_1  		(game_id)			REFERENCES games(id),
	INDEX 			Idx_game_players_1 		(game_id, player_name),
	INDEX 			Idx_game_players_2 		(game_id, round_position)
);


CREATE TABLE IF NOT EXISTS player_moves (
	id 					int UNSIGNED 		NOT NULL AUTO_INCREMENT,
	game_id 			int UNSIGNED 		NOT NULL,
	player_id			int UNSIGNED 		NOT NULL,
	player_name			varchar(100)		NOT NULL,
	previous_card		varchar(50)			NOT NULL,
	is_higher			BOOLEAN				NOT NULL,
	next_card			varchar(50)			NOT NULL,
	is_correct			BOOLEAN				NOT NULL,
	round_count			int					NOT NULL,
	created_at 			TIMESTAMP 			NOT NULL DEFAULT CURRENT_TIMESTAMP,
	updated_at 			TIMESTAMP 			NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT		Pk_player_moves 		PRIMARY KEY (id),
	FOREIGN KEY 	Fk_player_moves_1  		(game_id)			REFERENCES games(id),
	FOREIGN KEY 	Fk_player_moves_2  		(player_id)			REFERENCES game_players(id),
	INDEX 			Idx_player_moves_1 		(game_id, player_name)
);

