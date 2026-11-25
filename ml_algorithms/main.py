from database import Database
import sklearn


database = Database()
database.run()


testtree(database)


if __name__ == "__main__":
    raise SystemExit(main())
