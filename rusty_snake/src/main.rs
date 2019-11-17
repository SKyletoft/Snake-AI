use std::ops::{Add, Sub};

fn main() {
    println!("Hello, world!");
}

struct Game {
    size: Point,
    apple: Point,
    head: Point,
    tail: Vec<Point>,
    length: usize,
    turns: i32
}
#[derive(Clone)]
#[derive(Copy)]
#[derive(PartialEq)]
struct Point {
    x: usize,
    y: usize
}

impl Game {
    fn new (size: Point) -> Game {
        Game {
            size: size,
            apple: Point::new(0,0),
            head: Point::new(size.x / 2, size.y / 2),
            tail: Vec::new(),
            length: 2,
            turns: 0
        }
    }
    fn play_turn (&mut self, dir_index: usize) -> bool {
        let direction = Point::new(0,0); //ACTUALLY FIX THIS
        if self.turns > 200 {
            return false;
        }
        self.turns += 1;
        self.tail.push(self.head);
        self.head = self.head + direction;
        let hit_exception;
        if self.head == self.apple {
            self.length += 1;
            self.apple = new_apple();
            hit_exception = true;
        } else {
            hit_exception = false;
        }
        while self.tail.len() > self.length {
            self.tail.remove(0);
        }
        if !hit_exception || self.head.x < 0 || self.head.x > self.size.x || self.head.y < 0 || self.head.y > self.size.y {
            return false;
        }
        for part in self.tail.iter() {
            if *part == self.head {
                return false;
            }
        }

        true
    }
}

impl Point {
    fn new (x: usize, y: usize) -> Point {
        Point {x: x, y: y}
    }
}

impl Add for Point {
    type Output = Point;
    fn add (self, other: Point) -> Point {
        Point::new(self.x + other.x, self.y + other.y)
    }
}

impl Sub for Point {
    type Output = Point;
    fn sub (self, other:Point) -> Point {
        Point::new(self.x - other.x, self.y - other.y)
    }
}