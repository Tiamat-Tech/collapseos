TARGETS = forth
OBJS = vm.o
BLKPACK = ../tools/blkpack
BLKUNPACK = ../tools/blkunpack

.PHONY: all
all: $(TARGETS)

$(BLKPACK):
	$(MAKE) -C ../tools

.PHONY: $(BLKUNPACK)
$(BLKUNPACK): $(BLKPACK)

stage: stage.c $(OBJS) blkfs
	$(CC) stage.c $(OBJS) -o $@

blkfs: $(BLKPACK)
	$(BLKPACK) ../blk > $@

forth: forth.c $(OBJS) blkfs
	$(CC) forth.c $(OBJS) -lncurses -o $@

vm.o: vm.c
	$(CC) -DFBIN_PATH=\"`pwd`/forth.bin\" -DBLKFS_PATH=\"`pwd`/blkfs\" -c -o vm.o vm.c


.PHONY: updatebootstrap
updatebootstrap: stage xcomp.fs pack
	./stage < xcomp.fs > new.bin
	mv new.bin forth.bin

.PHONY: pack
pack:
	rm blkfs && $(MAKE) blkfs

.PHONY: unpack
unpack:
	$(BLKUNPACK) ../blk < blkfs

.PHONY: clean
clean:
	rm -f $(TARGETS) *.o blkfs