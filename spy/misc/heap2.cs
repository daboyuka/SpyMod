/////////////////
// Min-heap
/////////////////

// %heap.size
// %heap.elem[%i] (1-based)
function Heap::new() { return newObject("Heap", SimSet); }

function Heap::size(%heap) { return %heap.size|0; }
function Heap::min(%heap) { return %heap.elem[1]; }
function Heap::echo(%heap) {
	echo("Heap:");
	for (%i = 1; %i <= (%heap.size|0); %i++) {
		echo("  ", %i, ":", %heap.elem[%i]);
	}
}

function Heap::push(%heap, %value) {
	%heap.elem[%heap.size++] = %value;
	Heap::fixUp(%heap, %heap.size);
}

function Heap::swap(%heap, %i, %j) {
	%t = %heap.elem[%i];
	%heap.elem[%i] = %heap.elem[%j];
	%heap.elem[%j] = %t;
}

function Heap::less(%heap, %i, %j) {
	return getWord(%heap.elem[%i], 0) < getWord(%heap.elem[%j], 0);
}

function Heap::fixUp(%heap, %i) {
	while (%i > 1) {
		%p = (%i >> 1);
		if (Heap::less(%heap, %p, %i)) break;

		Heap::swap(%heap, %i, %p);
		%i = %p;
	}
}

function Heap::fixDown(%heap, %i) {
	%last = (%heap.size >> 1);
	while (%i <= %last) {
		%c = %i << 1;
		if (%c+1 <= %heap.size && %heap.elem[%c+1] < %heap.elem[%c])
			%c = %c+1;

		if (!Heap::less(%heap, %c, %i)) break;

		Heap::swap(%heap, %i, %c);
		%i = %c;
	}
}

function Heap::pop(%heap) {
	if (%heap.size < 1) return "";

	%min = Heap::min(%heap);

	Heap::swap(%heap, 1, %heap.size);
	%heap.elem[%heap.size] = "";
	%heap.size--;

	Heap::fixDown(%heap, 1);

	return %min;
}
